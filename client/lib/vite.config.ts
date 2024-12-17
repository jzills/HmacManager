import path from "path";
import { defineConfig } from "vite";
import dts from "vite-plugin-dts";

export default defineConfig({
    plugins: [
        dts({
            outDir: "dist",
            rollupTypes: true
        }),
    ],
    build: {
        lib: {
            entry: path.resolve(__dirname, "src/index.ts"),
            name: "HmacManager",
            fileName: "hmac-manager", 
        },
        rollupOptions: {
            output: [{
                entryFileNames: "hmac-manager.js",
                format: "esm", 
            },
            {
                entryFileNames: "hmac-manager.cjs",  
                format: "cjs",
            }],
            external: ["tslib", "node_modules"], 
        },
        target: "esnext"
    }
});