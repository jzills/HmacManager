import { Algorithms } from "./algorithms.js";
import { KeyCredentials } from "./key-credentials.js";
import { Nonce } from "./nonce.js";

export class HmacPolicy {
    readonly name: string;
    readonly keys: KeyCredentials;
    readonly algorithms: Algorithms;
    readonly nonce: Nonce;

    constructor(
        name: string, 
        keys: KeyCredentials, 
        algorithms: Algorithms, 
        nonce: Nonce
    ) {
        this.name = name;
        this.keys = keys;
        this.algorithms = algorithms;
        this.nonce = nonce;
    }
}