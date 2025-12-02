import * as readline from 'node:readline/promises';
import * as fs from "node:fs";

type NodeTaskFunc<TOutput = any, TInput = any> = (input: TInput) => Promise<TOutput | void>;
type NodeTaskRequest<TInput = any> = { id: string, name: string, input: TInput };
type NodeTaskResponse<TOutput = any> = { id: string, name: string, success: boolean, output?: TOutput, error?: any };

export class DotnetWorkerBuilder<TConfig = any, TAppSettings = any> {
    public readonly map: Map<string, NodeTaskFunc> = new Map()
    public readonly config: TConfig | undefined;
    public readonly appsettings: TAppSettings;

    private constructor() {
        const args = process.argv.splice(2).join(' ');
        this.config = args?.trim().length > 0 ? JSON.parse(args) : undefined;
        this.map.set("check", input => input);
        try {
            this.appsettings = JSON.parse(fs.readFileSync(`${__dirname}/../appsettings.json`).toString());
        } catch {
            this.appsettings = JSON.parse(fs.readFileSync(`${__dirname}/../Parto/appsettings.json`).toString());
        }
    }

    public static create<TConfig = any, TAppSettings = any>(): DotnetWorkerBuilder<TConfig, TAppSettings> {
        return new DotnetWorkerBuilder<TConfig, TAppSettings>();
    }

    add<TOutput = any, TInput = any>(name: string, action: NodeTaskFunc<TOutput, TInput>) {
        this.map.set(name, action);
    }

    log(message: string) {
        process.stdout.write(`log>${message}\n`)
    }

    build() {
        return new DotnetWorker<TConfig, TAppSettings>(this)
    }
}

export class DotnetWorker<TConfig = any, TAppSettings = any> {
    private readonly rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
        terminal: false
    });

    public get config(): TConfig | undefined {
        return this.builder.config;
    }

    public get appSettings(): TAppSettings {
        return this.builder.appsettings;
    }

    public get map(): Map<string, NodeTaskFunc> {
        return this.builder.map;
    }

    log(message: string) {
        this.builder.log(message);
    }

    constructor(private builder: DotnetWorkerBuilder<TConfig, TAppSettings>) {
    }

    async invoke<TOutput = any, TInput = any>(nodeTaskRequest: NodeTaskRequest<TInput>): Promise<NodeTaskResponse<TOutput> | undefined> {

        const func = this.map.get(nodeTaskRequest.name)

        if (!func) {
            return;
        }

        try {
            return {
                id: nodeTaskRequest.id,
                name: nodeTaskRequest.name,
                success: true,
                output: await func(nodeTaskRequest.input)
            }

        } catch (e) {
            return {
                id: nodeTaskRequest.id,
                name: nodeTaskRequest.name,
                success: false,
                error: JSON.stringify(e)
            }
        }
    }

    async run() {
        for await (const line of this.rl) {
            const input = line.trim();
            const prefix = "request>";
            if (!input.startsWith(prefix)) {
                this.log(`not have prefix: ${input}\n`)
                continue;
            }

            const nodeTaskRequest: NodeTaskRequest = JSON.parse(input.slice(prefix.length));
            const nodeTaskResponse: NodeTaskResponse | undefined = await this.invoke(nodeTaskRequest);

            if (!nodeTaskResponse) {
                this.log(`cant find task: ${input}\n`)
                continue;
            }

            process.stdout.write("response>" + JSON.stringify(nodeTaskResponse) + '\n');
        }
    }

    runSync() {
        this.run().catch(e => console.error(e));
    }
}
