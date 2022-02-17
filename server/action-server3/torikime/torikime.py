#!/usr/bin/env python3
 
import argparse
from jinja2 import Template, Environment, FileSystemLoader
import yaml

def main():
    parser = argparse.ArgumentParser(description='torikime')
    parser.add_argument('--namespace', type=str, default='torikime')
    parser.add_argument('-i', '--input_rpc', type=str)
    parser.add_argument('-o', '--out_dir', type=str)
    parser.add_argument('-n', '--dryrun', action='store_true')
    parser.add_argument('-s', '--show_outputs', action='store_true')
    args = parser.parse_args()

    print(f'input file={args.input_rpc}')
    print(f'out dir={args.out_dir}')

    env = Environment(loader=FileSystemLoader('./', encoding='utf8'))
    tmpl = env.get_template('proto.j2')

    with open(args.input_rpc) as file:
        contracts = yaml.safe_load(file)

        # print(contracts)

        for contract in contracts:
            # print(contract)
            # print(contracts[contract])
            for rpc in contracts[contract]:
                # print(rpc)
                # print(contracts[contract][rpc])
                params = {
                    "namespace": args.namespace,
                    "contract": contract,
                    "name": rpc,
                    "rpc": contracts[contract][rpc],
                }
                # print(params)

                rendered_s = tmpl.render(params)
                if(args.show_outputs):
                    print(rendered_s)

                if(not args.dryrun and args.out_dir):
                    with open(f'{args.out_dir}/{contract}_{rpc}.proto', mode='w') as f:
                        f.write(rendered_s)
                else:
                    print(f'{args.out_dir}/{contract}_{rpc}.proto')

if __name__ == "__main__":
    main()
