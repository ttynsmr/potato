#!/usr/bin/env python3
 
import argparse
import glob
import os

import inflection
import yaml
from jinja2 import Environment, FileSystemLoader, Template


def convert_rpc_to_protobuf(env, out_dir, params):
    tmpl = env.get_template('proto.j2')

    rendered_s = tmpl.render(params)

    os.makedirs(f'{out_dir}/{params["contract"]}/', exist_ok=True)
    with open(f'{out_dir}/{params["contract"]}/{params["contract"]}_{params["name"]}.proto', mode='w') as f:
        f.write(rendered_s)

def convert_rpc_to_hpp(env, out_dir, params):
    tmpl = env.get_template('rpc-hpp.j2')

    rendered_s = tmpl.render(params)

    os.makedirs(f'{out_dir}/cpp/', exist_ok=True)
    with open(f'{out_dir}/cpp/{params["contract"]}_{params["name"]}.h', mode='w') as f:
        f.write(rendered_s)

def convert_rpc_to_cpp(env, out_dir, params):
    tmpl = env.get_template('rpc-cpp.j2')

    rendered_s = tmpl.render(params)

    os.makedirs(f'{out_dir}/cpp/', exist_ok=True)
    with open(f'{out_dir}/cpp/{params["contract"]}_{params["name"]}.cpp', mode='w') as f:
        f.write(rendered_s)

def convert_rpc_to_csharp(env, out_dir, params):
    tmpl = env.get_template('rpc-csharp.j2')

    rendered_s = tmpl.render(params)

    os.makedirs(f'{out_dir}/', exist_ok=True)
    with open(f'{out_dir}/{inflection.camelize(params["contract"])}_{inflection.camelize(params["name"])}.cs', mode='w') as f:
        f.write(rendered_s)

def camelize(input):
    return inflection.camelize(input)

def main():
    parser = argparse.ArgumentParser(description='torikime')
    parser.add_argument('--namespace', type=str, default='torikime')
    parser.add_argument('-i', '--input_dir', type=str)
    parser.add_argument('-o', '--proto_out_dir', type=str)
    parser.add_argument('-c', '--cpp_out_dir', type=str)
    parser.add_argument('-s', '--csharp_out_dir', type=str)
    parser.add_argument('-n', '--dryrun', action='store_true')
    # parser.add_argument('-s', '--show_outputs', action='store_true')
    args = parser.parse_args()

    # print(f'input_dir={args.input_dir}')
    # print(f'proto_out_dir={args.proto_out_dir}')
    # print(f'cpp_out_dir={args.cpp_out_dir}')

    env = Environment(loader=FileSystemLoader('./', encoding='utf8'))

    env.filters['camelize'] = camelize

    tmpl = env.get_template('proto.j2')

    rpc_files = sorted(glob.glob(args.input_dir + '/*.yaml'))
    for contract_idx, rpc_file in enumerate(rpc_files):
        # print(f'{contract_idx}  {rpc_file}')

        with open(rpc_file) as file:
            file = yaml.safe_load(file)
            
            contracts = file['contracts']
            # print(contracts)
            if(not isinstance(contracts, dict)):
                return

            for contract in contracts:
                # print(contract)
                # print(contracts[contract])
                for rpc_idx, rpc in enumerate(contracts[contract]):
                    # print(rpc)
                    # print(contracts[contract][rpc])
                    params = {
                        "namespace": args.namespace,
                        "contract_id": contract_idx,
                        "rpc_id": rpc_idx,
                        "contract": contract,
                        "name": rpc,
                        "rpc": contracts[contract][rpc],
                    }
                    # print(params)

                    if(not args.dryrun):
                        if args.proto_out_dir:
                            convert_rpc_to_protobuf(env, args.proto_out_dir, params)
                        if args.cpp_out_dir:
                            convert_rpc_to_hpp(env, args.cpp_out_dir, params)
                            convert_rpc_to_cpp(env, args.cpp_out_dir, params)
                        if args.csharp_out_dir:
                            convert_rpc_to_csharp(env, args.csharp_out_dir, params)
                    else:
                        print(f'{args.out_dir}/{params["contract"]}/{params["contract"]}_{params["rpc"]}.proto')

if __name__ == "__main__":
    main()
