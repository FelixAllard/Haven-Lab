import { motion } from 'motion/react';

const HoverScaleWrapper = ({ children, scale = 1.1, className = '' }) => {
  return (
    <motion.div
      whileHover={{ scale }}
      transition={{ type: 'spring', stiffness: 200 }}
      className={className}
      style={{ display: 'inline-block', verticalAlign: 'middle' }} // Fix alignment issue
    >
      {children}
    </motion.div>
  );
};

export default HoverScaleWrapper;
