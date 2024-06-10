import { HmacManagerOptions } from "./components/hmac-manager-options.js";
import { HmacProvider } from "./components/hmac-provider.js";
import { HmacManager } from "./hmac-manager.js";

export class HmacManagerFactory {
    private readonly policies: any;
    private readonly caches: any;

    constructor(policies: any, caches: any | null) {
        this.policies = policies;
        this.caches = caches;
    }

    create(policy: string, scheme: string | null = null): HmacManager {
        return null;
        // return new HmacManager(
        //     new HmacManagerOptions(), 
        //     new HmacProvider()
        // );
    }
}