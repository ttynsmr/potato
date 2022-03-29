#include "user/user_registory.h"

#include "user/user_types.h"
#include "user/user.h"

namespace potato
{
	std::shared_ptr<User> UserRegistory::createUser()
	{
		return registerUser(UserId(++_currentUserId));
	}

	std::shared_ptr<User> UserRegistory::registerUser(UserId userId)
	{
		return _users.emplace_back(std::make_shared<User>(userId));
	}

	void UserRegistory::unregisterUser(std::shared_ptr<User> user)
	{
		_onUnregisterUser(user);
		_users.remove(user);
	}

	void UserRegistory::unregisterUserByUserId(UserId userId)
	{
		// 💩
		auto user = find(userId);
		if (user)
		{
			unregisterUser(user);
		}
	}

	std::shared_ptr<User> UserRegistory::find(UserId userId)
	{
		auto userIt = std::find_if(_users.begin(), _users.end(), [userId](auto& user) { return user->getUserId() == userId; });
		if (userIt == _users.end())
		{
			return std::shared_ptr<User>();
		}
		return *userIt;
	}

	void UserRegistory::update(int64_t now)
	{
		for (auto& user : _users)
		{
			user->update(now);
		}

		// 💩
		std::vector<std::shared_ptr<User>> removeUsers;
		std::copy_if(_users.begin(), _users.end(), std::back_inserter(removeUsers), [now](auto& user) { return user->isExpired(now); });
		for (auto& user : removeUsers)
		{
			unregisterUser(user);
		}
	}

	void UserRegistory::setOnUnregisterUser(OnUnregisterUserDelegate onUnregisterUser)
	{
		_onUnregisterUser = onUnregisterUser;
	}
}
