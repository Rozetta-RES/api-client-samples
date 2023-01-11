import time
import os
import json
import requests
import zipfile
import auth_utils
import config

def send_request(server_config, access_key, secret_key, translate_item_ids):
    item_ids = translate_item_ids.split(",")
    url = f"/api/v1/downloads?ids={json.dumps(item_ids)}"
    nonce = str(int(time.time()))
    signature = auth_utils.generate_signature(url, secret_key, nonce)

    headers = {
        "accessKey": access_key,
        "signature": signature,
        "nonce": nonce,
    }

    response = requests.get(
        f"{server_config['protocol']}//{server_config['hostname']}{url}",
        headers=headers
    )
    if response.status_code == 200:
        with open("./output.zip", "wb") as f:
            f.write(response.content)
        with zipfile.ZipFile("./output.zip") as zip_file:
            zip_file.extractall("./")
        os.remove("./output.zip")

def main():
    # response.json()['data']['items'][0]['translateItemId']
    # or
    # response.json()['data']['items'][0]['translateItemId'],response.json()['data']['items'][2]['translateItemId'],...
    translate_item_ids = "your translateItemId"
    try:
        send_request(
            config.server_config,
            config.auth_config['access_key'],
            config.auth_config['secret_key'],
            translate_item_ids
        )
    except Exception as error:
        print(error)

if __name__ == "__main__":
    main()
