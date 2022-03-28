#pragma once

#include <cstdint>

#include <strong_type/strong_type.hpp>
struct unit_id {};
using UnitId = strong::type<uint64_t, struct unit_id, strong::ordered, strong::equality, strong::hashable, strong::incrementable, strong::default_constructible>;

namespace boost
{
	inline size_t hash_value(const UnitId& v) { return std::hash<UnitId>()(v); }
}

#include <fmt/core.h>
#include <fmt/format.h>
template <> struct fmt::formatter<UnitId> : fmt::formatter<std::string>
{
	template <typename FormatContext>
	auto format(UnitId v, FormatContext& ctx) {
		return fmt::formatter<std::string>::format(fmt::to_string(v.value_of()), ctx);
	}
};
