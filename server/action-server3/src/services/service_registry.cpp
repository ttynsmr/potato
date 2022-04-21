#include "service_registry.h"

ServiceRegistry& ServiceRegistry::instance()
{
	static ServiceRegistry instance;
	return instance;
}
