import { HmacManager } from "./hmac-manager.js";
import { HmacPolicy } from "./components/hmac-policy.js";
import { HmacPolicyCollection } from "./components/hmac-policy-collection.js";

/**
 * A factory class for creating instances of HmacManager based on
 * specified policies and schemes.
 */
export class HmacManagerFactory {
    private readonly policies: HmacPolicyCollection;

    /**
     * Initializes the factory with a collection of HmacPolicy objects.
     * @param policies - Array of HmacPolicy objects to manage and retrieve.
     */
    constructor(policies: HmacPolicy[]) {
        this.policies = new HmacPolicyCollection(policies);
    }

    /**
     * Creates and returns an HmacManager instance based on the specified policy and scheme.
     * @param policy - Name of the policy to match.
     * @param scheme - Optional name of the scheme to match.
     * @returns An HmacManager instance if a matching policy is found; otherwise, null.
     */
    create(policy: string, scheme: string | null = null): HmacManager | null {
        const [matchingPolicy, matchingScheme] = this.policies.get(policy, scheme);
        if (matchingPolicy) {
            return new HmacManager(matchingPolicy, matchingScheme);
        } else {
            return null;
        }
    }
}