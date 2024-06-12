import { Question } from 'src/app/interfaces/question';
import { TestAttemptAnswer } from 'src/app/interfaces/test.attempt.answer';

export interface TestAttemptEntry {
  IsMultitipleChoise: boolean;
  Question: Question;
  Answers: TestAttemptAnswer[];
}
