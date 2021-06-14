#include"SignalBase.h"

using namespace Psycho;

void SignalBase::AddCommand(std::shared_ptr<CreatorFunction> creatorFunction)
{
	m_Commands.push_back(std::move(creatorFunction));
}

void SignalBase::Dispatch()
{
	for (int i = 0; i < m_Commands.size(); i++)
	{
		const CreatorFunction& creatorFunction = *(m_Commands.at(i).get());
		const auto object = std::static_pointer_cast<CommandBase>(creatorFunction());
		object->Execute();
	}
}