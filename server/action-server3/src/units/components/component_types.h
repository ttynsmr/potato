#pragma once

#include <memory>

class Unit;
namespace potato
{
	class Area;
}

class IComponent
{
public:
	IComponent() {}
	virtual ~IComponent() {};

	virtual void onSpawn(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/, std::shared_ptr<potato::Area> /*area*/) {}
	virtual void onDespawn(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/, std::shared_ptr<potato::Area> /*area*/) {}

	virtual void onConnected(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
	virtual void onDisconnected(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}

	virtual void preUpdate(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
	virtual void update(std::shared_ptr<Unit> /*unit*/, int64_t /*now*/) {}
};
