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

	template <typename T>
	std::shared_ptr<T> registerServiceProvider(std::shared_ptr<T> serviceProvider)
	{
		std::scoped_lock lock(_serviceProvidersLock);
		_serviceProviders.push_back(serviceProvider);
		return serviceProvider;
	}

	template <typename T>
	std::shared_ptr<T> findServiceProvider()
	{
		auto serviceProvider = [this] {
			std::scoped_lock lock(_serviceProvidersLock);
			return std::find_if(_serviceProviders.begin(), _serviceProviders.end(), [](std::shared_ptr<IServiceProvider>& s) {
				return std::dynamic_pointer_cast<T>(s); });
		}();

		if (serviceProvider == _serviceProviders.end())
		{
			return nullptr;
		}

		return std::dynamic_pointer_cast<T>(*serviceProvider);
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
	std::list<std::shared_ptr<IServiceProvider>> _serviceProviders;
	Queue queue;
};

