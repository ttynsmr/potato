#pragma once

#include <strong_type/strong_type.hpp>
struct user_id {};
//using UserId = strong::type<uint64_t, struct user_id, strong::ordered>;
using UserId = uint64_t;

#include <boost/multi_index_container.hpp>
#include <boost/multi_index/member.hpp>
#include <boost/multi_index/hashed_index.hpp>

#include "session/session_types.h"
#include "units/unit_types.h"

struct IdBinder
{
	UserId userId;
	potato::net::SessionId sessionId;
	UnitId unitId;
};

//struct user_id {};
struct session_id {};
struct unit_id {};

using IdLookupContainer = boost::multi_index_container<
	IdBinder,
	boost::multi_index::indexed_by<
	boost::multi_index::hashed_unique<boost::multi_index::tag<user_id>, boost::multi_index::member<IdBinder, UserId, &IdBinder::userId> >,
	boost::multi_index::hashed_unique<boost::multi_index::tag<session_id>, boost::multi_index::member<IdBinder, potato::net::SessionId, &IdBinder::sessionId> >,
	boost::multi_index::hashed_unique<boost::multi_index::tag<unit_id>, boost::multi_index::member<IdBinder, UnitId, &IdBinder::unitId> >	>
>;

class UserAuthenticator
{
public:
	std::optional<UserId> DoAuth(std::string id, std::string password)
	{
		return UserId(std::hash<std::string>()(id + password));
	}
};
