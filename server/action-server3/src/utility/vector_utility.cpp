#include "vector_utility.h"

#include "core/configured_eigen.h"
#include "proto/message.pb.h"

potato::Vector3* newVector3(const Eigen::Vector3f& v)
{
	auto nv = new potato::Vector3();
	nv->set_x(v.x());
	nv->set_y(v.y());
	nv->set_z(v.z());
	return nv;
}

Eigen::Vector3f toVector3f(const potato::Vector3& v)
{
	return Eigen::Vector3f({ v.x(), v.y(), v.z() });
}
