#pragma once
#include <memory>
#include <unordered_map>
#include <stdexcept>
#include <functional>
#include "SignalBase.h"
#include "UniquesKeyGenerator.h"

namespace Psycho
{
	using InstanceMap = std::unordered_map<int, std::shared_ptr<void>>;
	using ResolverMap = std::unordered_map<int, std::shared_ptr<CreatorFunction>>;
	using CommandMap = std::unordered_map<int, std::shared_ptr<SignalBase>>;

	template<class Key>
	class InstanceBindingAgent;

	template<class Key>
	class SignalBindingAgent;
	
	class IContext : public std::enable_shared_from_this<IContext>
	{
	private:
		InstanceMap m_InstanceMap;
		ResolverMap m_ResolverMap;
		CommandMap m_CommandMap;
	
	public:
		IContext() { }

		virtual void DoBindings() {}

		template<class Key>
		InstanceBindingAgent<Key> Bind();

		template<class Key , class Value , class... Dependencies, class... Args>
		void To(Args&& ... args);

		template<class Key>
		std::weak_ptr<Key>Resolve();

		template<class Signal>
		SignalBindingAgent<Signal> On();
		
		template<class Key, class Value, class... Dependencies, class... Args>
		void Do(Args&& ... args);
		
	};

	template<class Key>
	class InstanceBindingAgent
	{
		private:
			IContext* m_Context;
		public:
			explicit InstanceBindingAgent(IContext* context) { m_Context = context; }
			template<class Value , class... Dependencies, class... Args>
			void To(Args&&... args)
			{
				m_Context->To<Key, Value, Dependencies...>(std::forward<Args>(args)...);
			}
	};

	template<class Key>
	class SignalBindingAgent
	{
		private:
			IContext* m_Context;
		public:
			explicit SignalBindingAgent(IContext* context) { m_Context = context; }
			template<class Value, class... Dependencies, class... Args>
			void Do(Args&&... args)
			{
				m_Context->Do<Key, Value, Dependencies...>(std::forward<Args>(args)...);
			}
	};

	template<class Key>
	InstanceBindingAgent<Key> IContext::Bind()
	{
		const int keyId = UniqueKeyGenerator::Get<Key>();
		InstanceMap::iterator it = m_InstanceMap.find(keyId);

		if (it != m_InstanceMap.end())
		{
			it->second = nullptr;
		}
		else
		{
			m_InstanceMap.insert(std::make_pair(keyId, nullptr));
		}

		return InstanceBindingAgent<Key>(this);
	}

	template<class Key, class Value, class... Dependencies, class... Args>
	void IContext::To(Args&&... args)
	{
		const int keyId = UniqueKeyGenerator::Get<Key>();
		if (m_InstanceMap.find(keyId) != m_InstanceMap.end())
		{
			m_InstanceMap[keyId] = std::make_shared<Value>(Resolve<Dependencies>()...,args...);
		}
	}

	template<class Key>
	std::weak_ptr<Key> IContext::Resolve()
	{
		const int keyId = UniqueKeyGenerator::Get<Key>();
		InstanceMap::iterator it = m_InstanceMap.find(keyId);
		if (it != m_InstanceMap.end())
		{
			return std::static_pointer_cast<Key>(it->second);
		}

		throw std::runtime_error("No binding was found for type :" + std::string(typeid(Key).name()));
	}

	template<class Key>
	SignalBindingAgent<Key> IContext::On()
	{
		static_assert(std::is_base_of<SignalBase, Key>(), "Please derive your 'On' class from SignalBase");
		const int keyId = UniqueKeyGenerator::Get<Key>();
		InstanceMap::iterator it1 = m_InstanceMap.find(keyId);
		if (it1 == m_InstanceMap.end())
		{
			m_InstanceMap.insert(std::make_pair(keyId,std::make_shared<Key>()));
		}
		
		return SignalBindingAgent<Key>(this);
	}

	template<class Key, class Value, class... Dependencies, class... Args>
	void IContext::Do(Args&& ... args)
	{
		static_assert(std::is_base_of<CommandBase, Value>(), "Please derive your 'Do' class from CommandBase");
		const int keyId = UniqueKeyGenerator::Get<Key>();
		InstanceMap::iterator it = m_InstanceMap.find(keyId);

		if (it != m_InstanceMap.end())
		{
			CreatorFunction delegate = [this, args...]()->std::shared_ptr<Value>
			{
				return std::make_shared<Value>(this->Resolve<Dependencies>()..., args...);
			};

			(std::static_pointer_cast<Key>(it->second))->AddCommand(std::make_shared<CreatorFunction>(std::move(delegate)));
		}
	}
}

