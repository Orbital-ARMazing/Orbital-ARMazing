import type { NextApiRequest, NextApiResponse } from 'next';
import { Result } from 'types/api';
import { QuizFetch } from 'types/quiz';

import { currentSession } from '@helper/session';
import { fetchAllQuiz } from '@helper/quiz';
import { fetchEventByID } from '@helper/event';
import { EventFetch } from 'types/event';

const handler = async (req: NextApiRequest, res: NextApiResponse) => {
  const session = await currentSession(req);

  let result: Result = {
    status: false,
    error: '',
    msg: '',
  };

  if (session) {
    const qn = await fetchAllQuiz(session);
    const parsedQuiz: QuizFetch[] = [];

    if (qn && qn.status) {
      const questionData: QuizFetch[] = qn.msg as QuizFetch[];
      for (let q = 0; q < questionData.length; q += 1) {
        if (questionData[q]) {
          const quiz: QuizFetch = questionData[q];

          const visible = quiz.visible ? 'Yes' : 'No';
          const event = await fetchEventByID(quiz.eventID);
          const eventMsg = event.msg as EventFetch;
          const eventName: string = eventMsg.name;

          if (event.status) {
            const data: QuizFetch = {
              id: quiz.id,
              event: eventName,
              eventID: quiz.eventID,
              question: quiz.question,
              options: quiz.options,
              answer: quiz.answer,
              points: quiz.points,
              visible: quiz.visible,
              isVisible: visible,
            };

            parsedQuiz.push(data);
          }
        }
      }

      result = {
        status: true,
        error: null,
        msg: parsedQuiz,
      };
      res.status(200).send(result);
      res.end();
    } else {
      result = {
        status: false,
        error: 'Cannot get all quiz',
        msg: '',
      };
      res.status(200).send(result);
      res.end();
    }
  } else {
    result = { status: false, error: 'Unauthenticated', msg: '' };
    res.status(200).send(result);
    res.end();
  }
};

export default handler;