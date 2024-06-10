import { IHeaderScheme } from "./i-header-scheme.js";
import { Header } from "./header.js";

export class HeaderScheme implements IHeaderScheme {
    public readonly name: string;
    private readonly collection: Header[] = [];

    constructor(name: string) {
        this.name = name;
    }

    addHeader = (name: string, claimType: string | null = null): void => {
        this.collection.push({ name, claimType: claimType ?? name});
    }

    getHeaders = () => this.collection;
}