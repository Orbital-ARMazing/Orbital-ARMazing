import React from 'react';
import NextLink from 'next/link';
import { Flex, Icon, Text } from '@chakra-ui/react';

/**
 * This component renders individual navigation links for the sidebar
 */
export default function NavLink({ link, ...rest }) {
  const { label, icon, href } = link;

  return (
    <NextLink href={href} passHref>
      <Flex
        align='center'
        p='4'
        mx='4'
        borderRadius='lg'
        role='group'
        cursor='pointer'
        _hover={{
          bg: 'cyan.400',
          color: 'white',
        }}
        {...rest}
      >
        {icon && (
          <Icon
            mr='4'
            fontSize='16'
            _groupHover={{
              color: 'white',
            }}
            as={icon}
          />
        )}
        <Text fontSize='0.9rem'>{label}</Text>
      </Flex>
    </NextLink>
  );
}
