#pragma once

#include <strong_type/strong_type.hpp>
namespace potato::net
{
	struct session_id {};
	using SessionId = strong::type<int32_t, struct session_id, strong::ordered, strong::equality, strong::hashable>;
}

namespace boost
{
	inline size_t hash_value(const potato::net::SessionId& v) { return std::hash<potato::net::SessionId>()(v); }
}

#include <fmt/core.h>
#include <fmt/format.h>
template <> struct fmt::formatter<potato::net::SessionId> : fmt::formatter<std::string>
{
	template <typename FormatContext>
	auto format(potato::net::SessionId v, FormatContext& ctx) {
		return fmt::formatter<std::string>::format(fmt::to_string(v.value_of()), ctx);
	}
};
