#!/usr/bin/env python3
 
import argparse
import os
from jinja2 import Template, Environment, FileSystemLoader
import yaml
import glob

def main():
    parser = argparse.ArgumentParser(description='torikime')
    parser.add_argument('--namespace', type=str, default='torikime')
    parser.add_argument('-i', '--input_dir', type=str)
    parser.add_argument('-o', '--out_dir', type=str)
    parser.add_argument('-n', '--dryrun', action='store_true')
    parser.add_argument('-s', '--show_outputs', action='store_true')
    args = parser.parse_args()

    print(f'input file={args.input_dir}')
    print(f'out dir={args.out_dir}')

    env = Environment(loader=FileSystemLoader('./', encoding='utf8'))
    tmpl = env.get_template('proto.j2')

    rpc_files = sorted(glob.glob(args.input_dir + '/*.yaml'))
    for contract_idx, rpc_file in enumerate(rpc_files):
        print(f'{contract_idx}  {rpc_file}')

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

                    rendered_s = tmpl.render(params)
                    if(args.show_outputs):
                        print(rendered_s)

                    if(not args.dryrun and args.out_dir):
                        os.makedirs(f'{args.out_dir}/{contract}/', exist_ok=True)
                        with open(f'{args.out_dir}/{contract}/{contract}_{rpc}.proto', mode='w') as f:
                            f.write(rendered_s)
                    else:
                        print(f'{args.out_dir}/{contract}/{contract}_{rpc}.proto')

if __name__ == "__main__":
    main()
