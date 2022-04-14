#include "serialize_service_provider.h"

SerializeServiceProvider::SerializeServiceProvider(std::shared_ptr<ServiceRegistry> service) : _service(service)
{
}

bool SerializeServiceProvider::isRunning()
{
	return false;
}

void SerializeServiceProvider::start() {}

void SerializeServiceProvider::stop() {}
