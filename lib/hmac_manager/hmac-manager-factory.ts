import { HmacManager } from "./hmac-manager.js";
import { HmacPolicy } from "./components/hmac-policy.js";

export class HmacManagerFactory {
    private readonly policies: HmacPolicy[];

    constructor(policies: HmacPolicy[]) {
        this.policies = policies;
    }

    create(policy: string, scheme: string | null = null): HmacManager | null {
        const matchingPolicy = this.policies.find(_ => _.name === policy);
        if (matchingPolicy) {
            if (scheme) {
                const matchingScheme = matchingPolicy.schemes.find(_ => _.name === scheme);
                if (matchingScheme) {
                    return new HmacManager(
                        matchingPolicy,
                        matchingScheme
                    );
                } else {
                    return null;
                }
            } else {
                return new HmacManager(matchingPolicy);
            }
        } else {
            return null;
        }
    }
}