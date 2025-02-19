import { motion } from 'motion/react';

const HoverScaleWrapper = ({ children, scale = 1.1, className = "" }) => {
    return (
        <motion.div
            whileHover={{ scale }}
            transition={{ type: "spring", stiffness: 200 }}
            className={className}
        >
            {children}
        </motion.div>
    );
};

export default HoverScaleWrapper;
