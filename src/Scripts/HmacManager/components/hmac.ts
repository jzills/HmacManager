import { HeaderValue } from "../headers/header-value.js";

export class Hmac {
    dateRequested: Date = new Date();
    nonce: string = crypto.randomUUID();
    signingContent: string = null;
    headerValues: HeaderValue[] = [];
    signature: string = null;
}