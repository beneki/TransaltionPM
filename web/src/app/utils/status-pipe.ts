import { Pipe, PipeTransform } from '@angular/core';
import { Status } from './enums';

@Pipe({
    standalone: true,
    name: 'status'
})
export class StatusPipe implements PipeTransform {
    transform(value: number): string {
        return Status[value] || 'Unknown';  // Default to 'Unknown' if not found
    }
}