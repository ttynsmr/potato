#pragma once

#include <list>
#include <memory>

#include "core/configured_eigen.h"

class Unit;

namespace potato
{
	template<typename T>
	class IComponent2
	{
	public:
		IComponent2(std::shared_ptr<T> owner) : _owner(owner) {}
		virtual ~IComponent2() {}

	protected:
		std::shared_ptr<T> _owner;
	private:
		IComponent2() = delete;
	};

	class Node
		: public std::enable_shared_from_this<Node>
	{
	public:
		Node() {}
		virtual ~Node() {}

		template<typename T>
		std::shared_ptr<T> addNode(std::shared_ptr<T> node)
		{
			return _subNodes.emplace_back(node);
		}

		void clearNodes();

		template<typename T, class... Args>
		std::shared_ptr<T> addComponent(Args... args)
		{
			std::pair<std::unordered_map<std::size_t, std::shared_ptr<IComponent2<Node>>>::iterator, bool> result = _components.emplace(typeid(T).hash_code(), std::make_shared<T>(args...));
			if (!result.second)
			{
				return nullptr;
			}

			std::shared_ptr<IComponent2<Node>> i = result.first->second;

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

			return std::dynamic_pointer_cast<T>(found->second);
		}

		template<typename T>
		const std::shared_ptr<T> getComponent() const
		{
			return getComponent<T>();
		}

		template<typename T>
		void removeComponent()
		{
			_components.erase(typeid(T).hash_code());
		}

		void clearComponents();

		using Processor = std::function<void(std::shared_ptr<Node> node)>;
		void process(Processor processor);

	private:
		std::list<std::shared_ptr<Node>> _subNodes;
		std::unordered_map<std::size_t, std::shared_ptr<IComponent2<Node>>> _components;
	};

	class NodeRoot final : public Node
	{
	public:
		NodeRoot() {}
		~NodeRoot() {}
	};

	class PlaceableComponent : public IComponent2<Node>
	{
	public:
		PlaceableComponent(std::shared_ptr<Node> node) : IComponent2<Node>(node) {}
		virtual ~PlaceableComponent() {}

		Eigen::Vector3f position;
	};

	class VisibileComponent : public PlaceableComponent
	{
	public:
		VisibileComponent(std::shared_ptr<Node> node) : PlaceableComponent(node) {}
		virtual ~VisibileComponent() {}

		void setVisible(bool visible) { _visible = visible; }
		bool isVisible() const { return _visible; }

	private:
		bool _visible;
	};

	class TriggerableComponent final : public VisibileComponent
	{
	public:
		TriggerableComponent(std::shared_ptr<Node> node) : VisibileComponent(node) {}
		~TriggerableComponent() {}

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

		using TriggerDelegate = std::function<void(std::shared_ptr<Unit> unit, time_t now)>;
		void setOnTrigger(TriggerDelegate onTriggerCallback)
		{
			trigger = onTriggerCallback;
		}

		void invokeOnTrigger(std::shared_ptr<Unit> unit, time_t now)
		{
			if (trigger)
			{
				trigger(unit, now);
			}
		}

	private:
		TriggerDelegate trigger;
		TriggerableComponent() = default;
	};
}
