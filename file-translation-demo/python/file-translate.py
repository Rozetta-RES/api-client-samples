import os
import time
import requests
import json

import auth_utils
import config

def send_request(server_config, access_key, secret_key, contract_id):
    url = '/api/v1/file-translate'
    nonce = str(int(time.time()))
    signature = auth_utils.generate_signature(url, secret_key, nonce)

    headers = {
        "accessKey": access_key,
        "signature": signature,
        "nonce": nonce,
    }

    file_path = os.path.join(
        os.path.dirname(__file__),
        'sample-files',
        'testfile.docx'
    )

    with open(file_path, "rb") as f:
        files = {"files": ("testfile.docx", f)}

        data = {
            "targetLangs": json.dumps(['en']),
            "fieldId": "1",
            "contractId": contract_id,
        }

        response = requests.post(
            f"{server_config['protocol']}//{server_config['hostname']}:{server_config['port']}{url}",
            headers=headers,
            data=data,
            files=files
        )
    print(response.text)

def send_request_with_multiple_files(server_config, access_key, secret_key, contract_id):
    url = '/api/v1/file-translate'
    nonce = str(int(time.time()))
    signature = auth_utils.generate_signature(url, secret_key, nonce)

    headers = {
        "accessKey": access_key,
        "signature": signature,
        "nonce": nonce,
    }

    path = 'sample-files'
    filename1 = 'sample.docx'
    filename2 = 'testfile.docx'

    file_path1 = os.path.join(
        os.path.dirname(__file__),
        path,
        filename1
    )
    file_path2 = os.path.join(
        os.path.dirname(__file__),
        path,
        filename2
    )

    files = [
        ('files', (filename1, open(file_path1, 'rb'))),
        ('files', (filename2, open(file_path2, 'rb')))
    ]

    data = {
        "targetLangs": json.dumps(['en']),
        "fieldId": "1",
        "contractId": contract_id,
    }

    response = requests.post(
        f"{server_config['protocol']}//{server_config['hostname']}:{server_config['port']}{url}",
        headers=headers,
        data=data,
        files=files
    )
    print(response.text)

def main():
    try:
        send_request_with_multiple_files(
            config.server_config,
            config.auth_config['access_key'],
            config.auth_config['secret_key'],
            config.auth_config['contract_id'],
        )
    except Exception as error:
        print(error)

if __name__ == "__main__":
    main()