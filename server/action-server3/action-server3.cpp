#include <fmt/core.h>

#include "src/services/service_registry.h"
#include "src/services/network_service_provider.h"
#include "src/services/game_service_provider.h"
#include "src/services/serialize_service_provider.h"

int main(int argc, char *argv[])
{
	try
	{
		int32_t portNumber = 28888;
		if (argc != 2)
		{
			fmt::print("Usage: async_tcp_echo_server <port>\n");
		}
		else
		{
			portNumber = std::atoi(argv[1]);
		}

		auto network = ServiceRegistry::instance().registerServiceProvider<NetworkServiceProvider>(portNumber);
		auto game = ServiceRegistry::instance().registerServiceProvider<GameServiceProvider>();
		auto serialize = ServiceRegistry::instance().registerServiceProvider<SerializeServiceProvider>();

		network->start();
		game->start();
		serialize->start();
		ServiceRegistry::instance().run();
	}
	catch (std::exception &e)
	{
		fmt::print("Exception: {}\n", e.what());
	}

	return 0;
}
