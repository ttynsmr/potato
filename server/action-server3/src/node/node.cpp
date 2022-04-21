#include "node.h"

using namespace potato;

void Node::process(Processor processor)
{
	std::for_each(_subNodes.begin(), _subNodes.end(), processor);
}

void Node::clearNodes()
{
	_subNodes.clear();
}

void Node::clearComponents()
{
	_components.clear();
}
