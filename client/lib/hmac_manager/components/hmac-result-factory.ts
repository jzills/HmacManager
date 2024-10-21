import { HmacResult } from "./hmac-result.js";
import { Hmac } from "./hmac.js";

export class HmacResultFactory {
    private readonly policy: string;
    private readonly scheme: string | null;

    constructor(policy: string, scheme: string | null) {
        this.policy = policy;
        this.scheme = scheme;
    }

    success = (hmac: Hmac) => this.create(true, hmac);

    failure = () => this.create(false);

    private create = (isSuccess: boolean, hmac: Hmac | null = null): HmacResult => ({
        policy: this.policy,
        scheme: this.scheme,
        hmac,
        isSuccess,
        dateGenerated: new Date()
    });
}