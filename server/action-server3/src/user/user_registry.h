#pragma once

#include <memory>
#include <list>
#include <atomic>

#include "session/session_types.h"
#include "units/unit_types.h"
#include "user/user_types.h"


namespace potato
{
	class User;

	class UserRegistry
	{
	public:
		std::shared_ptr<User> createUser();
		std::shared_ptr<User> registerUser(UserId userId);

		void unregisterUser(std::shared_ptr<User> user);
		void unregisterUserByUserId(UserId userId);

		std::shared_ptr<User> find(UserId userId);
		std::shared_ptr<User> findByUnitId(UnitId unitId);

		void update(int64_t now);

		using OnUnregisterUserDelegate = std::function<void(std::shared_ptr<User>)>;
		void setOnUnregisterUser(OnUnregisterUserDelegate onUnregisterUser);

	private:
		std::list<std::shared_ptr<User>> _users;
		std::atomic<uint64_t> _currentUserId = 0;
		OnUnregisterUserDelegate _onUnregisterUser;
	};
}
