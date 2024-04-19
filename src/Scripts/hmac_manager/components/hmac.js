import { webcrypto } from "crypto";

export class Hmac {
    dateRequested = new Date();
    nonce = crypto.randomUUID();
    signingContent = null;
    signedHeaders = null;
    signature = null;
}