#pragma once

#include <memory>
#include <thread>
#include <eventpp/eventqueue.h>
#include "service_provider.h"

enum class ServiceProviderType
{
	Network,
	Game,
};

class ServiceRegistry final
{
public:
	static ServiceRegistry& instance();

	template<typename T, class... Args>
	std::shared_ptr<T> registerServiceProvider(Args... args)
	{
		std::pair<std::unordered_map<std::size_t, std::shared_ptr<IServiceProvider>>::iterator, bool> result = _components.emplace(typeid(T).hash_code(), std::make_shared<T>(args...));
		if (!result.second)
		{
			return nullptr;
		}

		std::shared_ptr<IServiceProvider> i = result.first->second;

		return std::dynamic_pointer_cast<T>(i);
	}

	template<typename T>
	std::shared_ptr<T> findServiceProvider()
	{
		auto found = _components.find(typeid(T).hash_code());
		if (found == _components.end())
		{
			return nullptr;
		}

		return std::dynamic_pointer_cast<T>(found->second);
	}

	void run()
	{
		while (running)
		{
			std::this_thread::sleep_for(std::chrono::seconds(1));
		}
	}

	using QueuedAction = std::function<void()>;
	using Queue = eventpp::EventQueue<ServiceProviderType, void(QueuedAction)>;
	Queue& getQueue()
	{
		return queue;
	}

private:
	ServiceRegistry() {}
	~ServiceRegistry() {}
	ServiceRegistry(const ServiceRegistry&) = delete;
	ServiceRegistry& operator=(const ServiceRegistry&) = delete;

	std::mutex _serviceProvidersLock;
	bool running = true;
	std::unordered_map<std::size_t, std::shared_ptr<IServiceProvider>> _components;
	Queue queue;
};

