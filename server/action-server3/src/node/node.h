#pragma once

#include <list>
#include <memory>

#include "core/configured_eigen.h"

namespace potato
{
	template<typename T>
	class IComponent2
	{
	public:
		IComponent2(std::shared_ptr<T> owner) {}
		virtual ~IComponent2() {}

	protected:
		IComponent2() = default;
	};

	class INode
	{
	public:
		INode() {}
		virtual ~INode() {}

		template<typename T>
		std::shared_ptr<T> addNode(std::shared_ptr<T> node)
		{
			return _subNodes.emplace_back(node);
		}

		template<typename T, class... Args>
		std::shared_ptr<T> addComponent(Args... args)
		{
			std::pair<std::unordered_map<std::size_t, std::shared_ptr<IComponent2<INode>>>::iterator, bool> result = _components.emplace(typeid(T).hash_code(), std::make_shared<T>(args...));
			if (!result.second)
			{
				return nullptr;
			}

			std::shared_ptr<IComponent2<INode>> i = result.first->second;

			return std::dynamic_pointer_cast<T>(i);
		}

		template<typename T>
		std::shared_ptr<T> getComponent()
		{
			auto found = _components.find(typeid(T).hash_code());
			if (found == _components.end())
			{
				return nullptr;
			}

			return *found;
		}

		template<typename T>
		void removeComponent()
		{
			_components.erase(typeid(T).hash_code());
		}

	private:
		std::list<std::shared_ptr<INode>> _subNodes;
		std::unordered_map<std::size_t, std::shared_ptr<IComponent2<INode>>> _components;
	};

	class NodeRoot final : public INode
	{
	public:
		NodeRoot() {}
		~NodeRoot() {}
	};

	class PlaceableComponent final : public IComponent2<INode>
	{
	public:
		PlaceableComponent(std::shared_ptr<INode>) {}
		~PlaceableComponent() {}

		Eigen::Vector3f position;

	private:
		PlaceableComponent() = default;
	};

	class TriggerableComponent final : public IComponent2<INode>
	{
	public:
		TriggerableComponent(std::shared_ptr<INode>) {}
		~TriggerableComponent() {}

		Eigen::Vector3f position;
		Eigen::Vector3f offset;
		Eigen::Vector3f size;

		bool containsAABB(const Eigen::Vector3f& point) const
		{
			const Eigen::Vector3f less = position + offset;
			const Eigen::Vector3f large = position + offset + size;

			assert(point.size() == less.size() && point.size() == large.size());
			for (int i = 0; i < point.size(); i++)
			{
				if ((less[i] > point[i]) && (point[i] > large[i]))
				{
					return false;
				}
			}
			return true;
		}

		using TriggerDelegate = std::function<void()>;
		void setOnTrigger(TriggerDelegate onTriggerCallback)
		{
			trigger = onTriggerCallback;
		}

		void invokeOnTrigger()
		{
			if (trigger)
			{
				trigger();
			}
		}

	private:
		TriggerDelegate trigger;
		TriggerableComponent() = default;
	};
}
