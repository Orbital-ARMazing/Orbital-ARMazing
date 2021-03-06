import type { NextApiRequest, NextApiResponse } from 'next';
import { Result } from 'types/api';
import { Leaderboard } from 'types/leaderboard';
import { fetchLeaderBoardByEventID } from '@helper/leaderboard';

/**
 * API Route to fetch the current Leaderboard filtered by Event ID
 *
 * This API route is only for the Unity Application
 * Authentication is exchanged through an Authorization Header code shared
 * between the two applications.
 *
 * @return A Result with status code
 */
const handler = async (req: NextApiRequest, res: NextApiResponse) => {
  let result: Result = {
    status: false,
    error: '',
    msg: '',
  };

  if (req.method === 'POST') {
    const { eventID } = req.body;

    if (
      req.headers.authorization !== null &&
      req.headers.authorization !== '' &&
      req.headers.authorization !== undefined
    ) {
      const head: string = req.headers.authorization;
      const secret: string = `Bearer ${process.env.AUTHORIZATION_HEADER}`;
      if (head === secret) {
        if (eventID) {
          const board: Result = await fetchLeaderBoardByEventID(eventID);
          if (board.status) {
            const leaderboard: Leaderboard[] = board.msg;
            result = {
              status: true,
              error: null,
              msg: leaderboard,
            };
            res.status(200).send(result);
            res.end();
          } else {
            result = {
              status: false,
              error: board.error,
              msg: [],
            };
            res.status(200).send(result);
            res.end();
          }
        } else {
          result = {
            status: false,
            error: 'No event ID',
            msg: [],
          };
          res.status(200).send(result);
          res.end();
        }
      } else {
        result = {
          status: false,
          error: 'Unauthorized, invalid token',
          msg: [],
        };
        res.status(401).send(result);
        res.end();
      }
    } else {
      result = {
        status: false,
        error: 'Unauthorized, token not found',
        msg: [],
      };
      res.status(401).send(result);
      res.end();
    }
  } else {
    result = {
      status: false,
      error: 'HTTP Post only',
      msg: '',
    };
    res.status(403).send(result);
    res.end();
  }
};

export default handler;
