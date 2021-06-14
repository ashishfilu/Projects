#pragma once

#include <vector>
#include <functional>
#include <memory>

namespace Psycho
{
	using CreatorFunction = std::function<std::shared_ptr<void>()>;

	class CommandBase
	{
	public:
		virtual void Execute() = 0;
	};

	class SignalBase
	{
	private:
		std::vector<std::shared_ptr<CreatorFunction>> m_Commands;

	public:
		void AddCommand(std::shared_ptr<CreatorFunction> creatorFunction);
		void Dispatch();
	};
}