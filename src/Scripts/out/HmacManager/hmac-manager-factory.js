export class HmacManagerFactory {
    policies;
    caches;
    constructor(policies, caches) {
        this.policies = policies;
        this.caches = caches;
    }
}
