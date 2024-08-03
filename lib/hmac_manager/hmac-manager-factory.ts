import { HmacManager } from "./hmac-manager.js";
import { HmacPolicy } from "./components/hmac-policy.js";
import { HmacPolicyCollection } from "./components/hmac-policy-collection.js";

export class HmacManagerFactory {
    private readonly policies: HmacPolicyCollection;

    constructor(policies: HmacPolicy[]) {
        this.policies = new HmacPolicyCollection(policies);
    }

    create(policy: string, scheme: string | null = null): HmacManager | null {
        const [matchingPolicy, matchingScheme] = this.policies.get(policy, scheme);
        if (matchingPolicy) {
            return new HmacManager(matchingPolicy, matchingScheme);
        } else {
            return null;
        }
    }
}