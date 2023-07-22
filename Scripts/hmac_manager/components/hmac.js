import { webcrypto } from "crypto";

export class Hmac {
    requestedOn = new Date();
    nonce = crypto.randomUUID();
    signingContent = null;
    signedHeaders = null;
    signature = null;
}