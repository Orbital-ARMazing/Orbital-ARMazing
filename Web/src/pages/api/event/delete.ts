import type { NextApiRequest, NextApiResponse } from 'next';
import { Result } from 'types/api';

import { currentSession } from '@helper/session';
import { deleteEvent, isCreatorOfEvent } from '@helper/event';
import { levels } from '@constants/admin';
import { checkerString } from '@helper/common';

const handler = async (req: NextApiRequest, res: NextApiResponse) => {
  const session = await currentSession(req);

  const { id } = req.body;
  let result: Result = {
    status: false,
    error: '',
    msg: '',
  };

  if (session) {
    if (session.user.level === levels.ORGANIZER) {
      if (checkerString(id)) {
        const isCreator: boolean = await isCreatorOfEvent(id, session);
        if (isCreator) {
          const event: Result = await deleteEvent(id);
          if (event.status) {
            result = {
              status: true,
              error: null,
              msg: 'Event deleted',
            };

            res.status(200).send(result);
            res.end();
          } else {
            result = {
              status: false,
              error: event.error,
              msg: '',
            };
            res.status(200).send(result);
            res.end();
          }
        } else {
          result = {
            status: false,
            error: 'Only the creator can delete the event',
            msg: null,
          };

          res.status(200).send(result);
          res.end();
        }
      } else {
        result = {
          status: false,
          error: 'Information incomplete!',
          msg: null,
        };

        res.status(200).send(result);
        res.end();
      }
    } else {
      result = {
        status: false,
        error: 'Unauthorized access',
        msg: null,
      };

      res.status(200).send(result);
      res.end();
    }
  } else {
    result = {
      status: false,
      error: 'Unauthorized access',
      msg: null,
    };

    res.status(200).send(result);
    res.end();
  }
};

export default handler;