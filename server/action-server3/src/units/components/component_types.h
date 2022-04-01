#pragma once

#include <memory>

class Unit;

class IComponent
{
public:
	IComponent() {}
	virtual ~IComponent() {}

	virtual void update(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
};
