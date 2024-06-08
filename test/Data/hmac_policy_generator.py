import uuid
import random
import base64
import json
import string

NUM_POLICIES = 1000

def hmac_policy_generator():
    """
    Generates a single hmac policy object.
    """

    def generate_uuid():
        """
        Generates an UUID public key.
        """

        return str(uuid.uuid4())

    def generate_base64_string():
        """
        Generates a base64 encoded private key.
        """
        
        return base64.b64encode(uuid.uuid4().bytes).decode('utf-8')

    def generate_content_hash_algorithm():
        """
        Generates a random content hash algorithm.
        """
        
        return random.choice(["SHA1", "SHA256", "SHA512"])

    def generate_signing_algorithm():
        """
        Generates a random signing hash algorithm.
        """

        return random.choice(["HMACSHA1", "HMACSHA256", "HMACSHA512"])

    def generate_cache_type():
        """
        Generates a random cache type.
        """

        return random.choice(["Memory", "Distributed"])

    def generate_non_empty_string():
        """
        Generates a random non empty string.
        """

        allowed_chars = string.ascii_letters + string.digits + '-_'
        length = random.randint(5, 15)  # Generate strings of length between 5 and 15
        return ''.join(random.choices(allowed_chars, k=length))

    def generate_headers():
        """
        Generates a randomized variable length set of headers.
        """

        headers = []
        for _ in range(random.randint(1, 5)):
            headers.append({
                "Name": generate_non_empty_string(),
                "ClaimType": generate_non_empty_string()
            })
            
        return headers

    random_data = {
        "Name": generate_non_empty_string(),
        "Keys": {
            "PublicKey": generate_uuid(),
            "PrivateKey": generate_base64_string()
        },
        "Algorithms": {
            "ContentHashAlgorithm": generate_content_hash_algorithm(),
            "SigningHashAlgorithm": generate_signing_algorithm()
        },
        "Nonce": {
            "CacheType": generate_cache_type(),
            "MaxAgeInSeconds": random.randint(10, 500)
        },
        "HeaderSchemes": [
            {
                "Name": generate_non_empty_string(),
                "Headers": generate_headers()
            }
        ]
    }

    # Randomize the order of properties
    random_keys = list(random_data.keys())
    random.shuffle(random_keys)
    randomized_data = {key: random_data[key] for key in random_keys}

    return randomized_data

# Generate and print the randomized data
with open("hmac_policy_collection.json", "w") as file:
    data = [hmac_policy_generator() for i in range(NUM_POLICIES)]
    file.write(json.dumps(data, indent=4))