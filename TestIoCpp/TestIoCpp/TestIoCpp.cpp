// TestIoCpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "TestContext.h"
#include <memory>

using namespace Psycho;
int main()
{
	std::unique_ptr<TestContext> temp = std::make_unique<TestContext>();
	temp->DoBindings();

	std::shared_ptr<TestSignal> signal = temp->Resolve<TestSignal>().lock();
	if (signal != nullptr)
	{
		signal->Dispatch();
	}
}

