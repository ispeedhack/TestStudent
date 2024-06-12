import { TestAttemptEntry } from 'src/app/interfaces/test.attempt.entry';

export interface TestAttempt {
  TestId: number;
  Title: string;
  TestAttemptEntries: TestAttemptEntry[];
}
