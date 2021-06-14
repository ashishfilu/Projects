#pragma once
#include "IContext.h"
#include "TestObject1.h"
#include "TestObject2.h"

namespace Psycho
{
	class TestSignal :public SignalBase{};
	class TestCommand1 : public CommandBase
	{
	public:
		TestCommand1() {}
		void Execute()override
		{
			std::cout << "Command 1 is triggered";
		}
	};

	class TestCommand2 : public CommandBase
	{
	public:
		TestCommand2(std::weak_ptr<TestObject2> temp) {}
		void Execute()override
		{
			std::cout << "Command 2 is triggered";
		}
	};

	class TestContext : public IContext
	{
	private:

	public:

		TestContext() {}
		
		virtual void DoBindings()override
		{
			Bind<TestObject1>().To<TestObject1>();
			Bind<TestObject2>().To<TestObject2,TestObject1>();
			On<TestSignal>().Do<TestCommand1>();
			On<TestSignal>().Do<TestCommand2, TestObject2>();
		}
	};
}