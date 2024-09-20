import { Project } from './project.model';
import { Translator } from './translator.model';

export class Task {
    id!: string;
    title!: string;
    description!: string;
    dueDate!: string;
    status!: number;
    projectId!: Project;
    translator!: Translator;
}
