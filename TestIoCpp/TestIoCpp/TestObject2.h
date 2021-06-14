#pragma once
#include<iostream>
#include<memory>

namespace Psycho
{
	class TestObject1;

	class TestObject2
	{
	private:
		std::weak_ptr<TestObject1> m_TestObject1;
	public:

		TestObject2(std::weak_ptr<TestObject1> testObject1)
		{
			m_TestObject1 = testObject1;
			std::cout << "Test object 2 is created";
		}
	};
}