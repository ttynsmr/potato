#!/usr/bin/env python3

import argparse
import glob
import hashlib
import os

import inflection
import yaml
from jinja2 import Environment, FileSystemLoader, Template


def is_cached(cache_file, new_data, args):
    new_hash = hashlib.sha256(new_data.encode("utf-8")).hexdigest()
    if args.cache_dir:
        if os.path.exists(cache_file):
            with open(cache_file, mode="r") as f:
                old_hash = f.read()
                if old_hash == new_hash:
                    # if args.verbose:
                    #     print(f"cache hit: {cache_file}")
                    return True
                else:
                    if args.verbose:
                        print(f"cache chk: {cache_file}: {old_hash} == {new_hash}")

    return False


def write_cache(cache_file, new_data, args):
    new_hash = hashlib.sha256(new_data.encode("utf-8")).hexdigest()
    os.makedirs(os.path.dirname(cache_file), exist_ok=True)
    with open(cache_file, mode="w") as f:
        f.write(new_hash)


def convert_rpc_to_protobuf(env, out_dir, params, args):
    tmpl = env.get_template("proto.j2")

    rendered_s = tmpl.render(params)

    filename = f'{params["contract"]}_{params["name"]}.proto'
    out_filename = f"{out_dir}/{filename}"
    cache_filename = f"{args.cache_dir}/{filename}.hash"
    if is_cached(cache_filename, rendered_s, args):
        return

    if args.verbose:
        print(f"output: {out_filename}")

    os.makedirs(os.path.dirname(out_filename), exist_ok=True)
    with open(out_filename, mode="w") as f:
        f.write(rendered_s)

    write_cache(cache_filename, rendered_s, args)


def convert_rpc_to_hpp(env, out_dir, params, args):
    tmpl = env.get_template("rpc-hpp.j2")

    rendered_s = tmpl.render(params)

    filename = f'cpp/{params["contract"]}_{params["name"]}.h'
    out_filename = f"{out_dir}/{filename}"
    cache_filename = f"{args.cache_dir}/{filename}.hash"
    if is_cached(cache_filename, rendered_s, args):
        return

    if args.verbose:
        print(f"output: {out_filename}")

    os.makedirs(os.path.dirname(out_filename), exist_ok=True)
    with open(out_filename, mode="w") as f:
        f.write(rendered_s)

    write_cache(cache_filename, rendered_s, args)


def convert_rpc_to_cpp(env, out_dir, params, args):
    tmpl = env.get_template("rpc-cpp.j2")

    rendered_s = tmpl.render(params)

    filename = f'cpp/{params["contract"]}_{params["name"]}.cpp'
    out_filename = f"{out_dir}/{filename}"
    cache_filename = f"{args.cache_dir}/{filename}.hash"
    if is_cached(cache_filename, rendered_s, args):
        return

    if args.verbose:
        print(f"output: {out_filename}")

    os.makedirs(os.path.dirname(out_filename), exist_ok=True)
    with open(out_filename, mode="w") as f:
        f.write(rendered_s)

    write_cache(cache_filename, rendered_s, args)


def convert_rpc_builder_to_cpp(env, out_dir, all_params, args):
    tmpl = env.get_template("rpc-builder-cpp.j2")

    rendered_s = tmpl.render(all_params)

    filename = f"cpp/rpc_builder.cpp"
    out_filename = f"{out_dir}/{filename}"
    cache_filename = f"{args.cache_dir}/{filename}.hash"
    if is_cached(cache_filename, rendered_s, args):
        print(out_filename)
        return

    if args.verbose:
        print(f"output: {out_filename}")

    os.makedirs(os.path.dirname(out_filename), exist_ok=True)
    with open(out_filename, mode="w") as f:
        f.write(rendered_s)

    write_cache(cache_filename, rendered_s, args)


def convert_rpc_builder_to_hpp(env, out_dir, all_params, args):
    tmpl = env.get_template("rpc-builder-hpp.j2")

    rendered_s = tmpl.render(all_params)

    filename = f"cpp/rpc_builder.h"
    out_filename = f"{out_dir}/{filename}"
    cache_filename = f"{args.cache_dir}/{filename}.hash"
    if is_cached(cache_filename, rendered_s, args):
        return

    if args.verbose:
        print(f"output: {out_filename}")

    os.makedirs(os.path.dirname(out_filename), exist_ok=True)
    with open(out_filename, mode="w") as f:
        f.write(rendered_s)

    write_cache(cache_filename, rendered_s, args)


def convert_rpc_to_csharp(env, out_dir, params, args):
    tmpl = env.get_template("rpc-csharp.j2")

    rendered_s = tmpl.render(params)

    filename = f'{inflection.camelize(params["contract"])}_{inflection.camelize(params["name"])}.cs'
    out_filename = f"{out_dir}/{filename}"
    cache_filename = f"{args.cache_dir}/{filename}.hash"
    if is_cached(cache_filename, rendered_s, args):
        return

    if args.verbose:
        print(f"output: {out_filename}")

    os.makedirs(os.path.dirname(out_filename), exist_ok=True)
    with open(out_filename, mode="w") as f:
        f.write(rendered_s)

    write_cache(cache_filename, rendered_s, args)


def camelize(input):
    return inflection.camelize(input)


def lower_camelize(input):
    return inflection.camelize(input, False)


def params_to(inpit_params):
    parameters = {}
    # print(inpit_params)
    for k, v in inpit_params.items():
        # print(
        #     k,
        #     v,
        #     "type" in inpit_params[k],
        # )
        parameters[k] = {}
        if "type" in inpit_params[k]:
            if inpit_params[k]["container"] == "array":
                parameters[k]["container"] = "array"
                parameters[k]["type"] = inpit_params[k]["type"]
            else:
                parameters[k]["container"] = "value"
                parameters[k]["type"] = v
        else:
            parameters[k]["container"] = "value"
            parameters[k]["type"] = v
    return parameters


def main():
    parser = argparse.ArgumentParser(description="torikime")
    parser.add_argument("--namespace", type=str, default="torikime")
    parser.add_argument("-i", "--input_dir", type=str)
    parser.add_argument("-o", "--proto_out_dir", type=str)
    parser.add_argument("-c", "--cpp_out_dir", type=str)
    parser.add_argument("-s", "--csharp_out_dir", type=str)
    parser.add_argument("-n", "--dryrun", action="store_true")
    parser.add_argument("-v", "--verbose", action="store_true")
    parser.add_argument("--cache_dir", type=str)
    # parser.add_argument('-s', '--show_outputs', action='store_true')
    args = parser.parse_args()

    # print(f'input_dir={args.input_dir}')
    # print(f'proto_out_dir={args.proto_out_dir}')
    # print(f'cpp_out_dir={args.cpp_out_dir}')

    env = Environment(loader=FileSystemLoader("./", encoding="utf8"))

    env.filters["camelize"] = camelize
    env.filters["lower_camelize"] = lower_camelize

    tmpl = env.get_template("proto.j2")

    all_params = {}
    all_params["namespace"] = args.namespace
    all_params["contracts"] = []
    rpc_files = sorted(glob.glob(args.input_dir + "/*.yaml"))
    for contract_idx, rpc_file in enumerate(rpc_files):
        if args.verbose:
            print(f"{contract_idx}  {rpc_file}")

        with open(rpc_file) as file:
            file = yaml.safe_load(file)

            contracts = file["contracts"]
            # print(contracts)
            if not isinstance(contracts, dict):
                return

            for contract in contracts:
                # print(contract)
                # print(contracts[contract])
                c = {}
                c["name"] = contract
                c["names"] = []
                all_params["contracts"].append(c)
                for rpc_idx, rpc in enumerate(contracts[contract]):
                    # print(rpc)
                    # print(contracts[contract][rpc])

                    rpc_def = {}
                    if "request" in contracts[contract][rpc]:
                        rpc_def["request"] = params_to(
                            contracts[contract][rpc]["request"]
                        )
                    if "response" in contracts[contract][rpc]:
                        rpc_def["response"] = params_to(
                            contracts[contract][rpc]["response"]
                        )
                    if "notification" in contracts[contract][rpc]:
                        rpc_def["notification"] = params_to(
                            contracts[contract][rpc]["notification"]
                        )

                    if "request" in contracts[contract][rpc]:
                        c["names"].append(rpc)

                    params = {
                        "namespace": args.namespace,
                        "contract_id": contract_idx,
                        "rpc_id": rpc_idx,
                        "contract": contract,
                        "name": rpc,
                        "rpc": contracts[contract][rpc],
                        "rpc_def": rpc_def,
                    }

                    if "imports" in contracts[contract][rpc]:
                        params["imports"] = contracts[contract][rpc]["imports"]

                    # print(params)

                    if not args.dryrun:
                        if args.proto_out_dir:
                            convert_rpc_to_protobuf(
                                env, args.proto_out_dir, params, args
                            )
                        if args.cpp_out_dir:
                            convert_rpc_to_hpp(env, args.cpp_out_dir, params, args)
                            convert_rpc_to_cpp(env, args.cpp_out_dir, params, args)
                        if args.csharp_out_dir:
                            convert_rpc_to_csharp(
                                env, args.csharp_out_dir, params, args
                            )
                    else:
                        print(
                            f'{args.out_dir}/{params["contract"]}/{params["contract"]}_{params["rpc"]}.proto'
                        )

    print(all_params)
    convert_rpc_builder_to_hpp(env, args.cpp_out_dir, all_params, args)
    convert_rpc_builder_to_cpp(env, args.cpp_out_dir, all_params, args)


if __name__ == "__main__":
    main()
