import time
import requests

import config
import auth_utils


def send_request(server_config, access_key, secret_key, translate_id):
    url = f"/api/v1/translate-result/{translate_id}"
    nonce = str(int(time.time()))
    signature = auth_utils.generate_signature(url, secret_key, nonce)

    headers = {
        "accessKey": access_key,
        "signature": signature,
        "nonce": nonce,
    }

    response = requests.get(
        f"{server_config['protocol']}//{server_config['hostname']}:{server_config['port']}{url}",
        headers=headers
    )
    print(response.text)

def main():
    try:
        send_request(
            config.server_config,
            config.auth_config['access_key'],
            config.auth_config['secret_key'],
            "your translateId", # response.json()['data']['translateId']
        )
    except Exception as error:
        print(error)

if __name__ == "__main__":
    main()
