import { HmacResult } from "./hmac-result.js";
import { Hmac } from "./hmac.js";

export class HmacResultFactory {
    policy: string;
    scheme: string;

    constructor(policy: string, scheme: string | null) {
        this.policy = policy;
        this.scheme = scheme;
    }

    create(hmac: Hmac, isSuccess: boolean): HmacResult {
        const result = new HmacResult(hmac, isSuccess);
        result.policy = this.policy;
        result.scheme = this.scheme;
        result.dateGenerated = new Date();
        return result;
    }

    error(information: string): HmacResult {
        const result = new HmacResult(null, false);
        result.information = information;
        result.dateGenerated = new Date();
        return result;
    }
}