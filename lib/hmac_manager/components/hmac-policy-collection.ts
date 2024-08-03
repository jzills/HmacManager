import { HmacPolicy } from "./hmac-policy.js";
import { HmacScheme } from "./hmac-scheme.js";

export class HmacPolicyCollection {
    private readonly policyCollection: { [key: string]: HmacPolicy } = {};
    private readonly schemeCollection: { [key: string]: HmacScheme } = {};

    constructor(policies: HmacPolicy[]) {
        for (const policy of policies) {
            this.policyCollection[policy.name] = policy;

            for (const scheme of policy.schemes) {
                this.schemeCollection[`${policy.name}:${scheme.name}`] = scheme;
            }
        }
    }

    get(policy: string, scheme: string | null = null): [HmacPolicy | null, HmacScheme | null] {
        const matchingPolicy = this.policyCollection[policy];
        const matchingScheme = this.schemeCollection[`${policy}:${scheme}`];
        return [matchingPolicy, matchingScheme];
    }
}