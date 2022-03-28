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
		return std::make_shared<User>(userId);
	}

	void UserRegistory::unregisterUser(std::shared_ptr<User> user)
	{
		_users.remove(user);
	}

	void UserRegistory::unregisterUserByUserId(UserId userId)
	{
		_users.remove_if([userId](auto& user) { return user->getUserId() == userId; });
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
	}
}
