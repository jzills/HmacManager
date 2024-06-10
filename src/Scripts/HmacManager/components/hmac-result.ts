import { Hmac } from "./hmac.js";

export class HmacResult {
    policy: string;
    scheme: string;
    hmac: Hmac;
    isSuccess: boolean;
    dateGenerated: Date;
    information: string;

    constructor(hmac, isSuccess) {
        this.hmac = hmac
        this.isSuccess = isSuccess
    }
}