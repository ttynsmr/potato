#pragma once

#include <memory>

class Unit;

class IComponent
{
public:
	IComponent() {}
	virtual ~IComponent() {};

	virtual void onSpawn(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
	virtual void onDespawn(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}

	virtual void onConnected(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
	virtual void onDisconnected(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}

	virtual void preUpdate(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
	virtual void update(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
};
