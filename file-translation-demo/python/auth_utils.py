import hashlib
import hmac

def generate_signature(path, secret_key, nonce):
    signature = hmac.new(secret_key.encode(), (nonce+path).encode(), hashlib.sha256)
    return signature.hexdigest()

